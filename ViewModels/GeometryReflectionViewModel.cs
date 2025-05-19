using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GeometryReflectionApp.Services;

namespace GeometryReflectionApp.ViewModels
{
    public partial class GeometryReflectionViewModel : ObservableObject
    {
        private readonly ReflectionService _reflectionService = new();
        private string _assemblyPath = string.Empty;
        private string _output = string.Empty;
        private string _currentShapeInfo = string.Empty;
        
        [ObservableProperty] private bool _hasSelectedType;
        [ObservableProperty] private bool _hasParameters;
        [ObservableProperty] private bool _hasSelectedMethod;
        [ObservableProperty] private bool _showBoundingRectEditor;
        [ObservableProperty] private double _boundingRectX1;
        [ObservableProperty] private double _boundingRectY1;
        [ObservableProperty] private double _boundingRectX2;
        [ObservableProperty] private double _boundingRectY2;

        private Type? _selectedType;
        private MethodInfo? _selectedMethod;
        private Shape? _currentShape;

        public ObservableCollection<Type> ShapeTypes { get; } = new();
        public ObservableCollection<MethodInfo> ShapeMethods { get; } = new();
        public ObservableCollection<MethodParameterViewModel> MethodParameters { get; } = new();

        public string CurrentShapeInfo
        {
            get => _currentShapeInfo;
            set => SetProperty(ref _currentShapeInfo, value);
        }

        public string AssemblyPath
        {
            get => _assemblyPath;
            set => SetProperty(ref _assemblyPath, value);
        }

        public string Output
        {
            get => _output;
            set => SetProperty(ref _output, value);
        }

        public Type? SelectedType
        {
            get => _selectedType;
            set => SetSelectedType(value);
        }

        public MethodInfo? SelectedMethod
        {
            get => _selectedMethod;
            set => SetSelectedMethod(value);
        }

        private void UpdateShapeInfo()
        {
            if (_currentShape != null)
            {
                var rect = _currentShape.GetBoundingRectangle();
                string sizeInfo = _currentShape is Rectangle r ? 
                    $"\nSize: {r.Width}x{r.Height}" : "";
                
                CurrentShapeInfo = $"Type: {_currentShape.GetType().Name}\n" +
                                $"Center: ({_currentShape.CenterX}, {_currentShape.CenterY})" +
                                sizeInfo +
                                $"\nBounding rect: ({rect.x1}, {rect.y1}) - ({rect.x2}, {rect.y2})\n" +
                                $"Area: {_currentShape.GetArea()}";
            }
        }

        [RelayCommand]
        private void LoadAssembly()
        {
            try
            {
                ClearAll();
                if (string.IsNullOrWhiteSpace(AssemblyPath))
                {
                    Output = "Specify the path to DLL";
                    return;
                }

                var assembly = _reflectionService.LoadAssembly(AssemblyPath);
                if (assembly == null)
                {
                    Output = "Failed to load the assembly";
                    return;
                }

                var types = _reflectionService.GetShapeTypes(assembly).ToList();
                if (!types.Any())
                {
                    Output = "The assembly doesn't contain any geometric shape classes";
                    return;
                }

                foreach (var type in types)
                    ShapeTypes.Add(type);

                Output = $"Loaded shape classes: {ShapeTypes.Count}";
            }
            catch (Exception ex)
            {
                Output = $"Error loading the assembly: {ex.Message}";
            }
        }

        [RelayCommand]
        private void CreateShape()
        {
            if (SelectedType == null) 
            {
                Output = "No shape type selected";
                return;
            }

            try
            {
                var constructor = SelectedType.GetConstructors().FirstOrDefault();
                if (constructor == null)
                {
                    Output = $"No public constructor found for {SelectedType.Name}";
                    return;
                }

                var parameters = constructor.GetParameters()
                    .Select(p => 
                    {
                        if (p.ParameterType.IsValueType)
                            return Activator.CreateInstance(p.ParameterType);
                        return null;
                    })
                    .ToArray();

                _currentShape = _reflectionService.CreateShape(SelectedType, parameters);
                
                if (_currentShape != null)
                {
                    UpdateShapeInfo();
                    Output = $"{SelectedType.Name} was created successfully";
                    LoadMethods();
                }
                else
                {
                    Output = $"Failed to create {SelectedType.Name}";
                }
            }
            catch (Exception ex)
            {
                Output = $"Error creating shape: {ex.Message}";
                Console.WriteLine($"Error creating shape: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ExecuteMethod()
        {
            if (_currentShape == null || SelectedMethod == null)
            {
                Output = "First, create the shape and select a method";
                return;
            }

            try
            {
                object?[] parameters = MethodParameters
                    .Select(p => p.Value != null ? Convert.ChangeType(p.Value, p.Type) : null)
                    .ToArray();

                var result = _reflectionService.InvokeMethod(_currentShape, SelectedMethod, parameters);
                Output = result?.ToString() ?? "Method executed successfully";
                UpdateShapeInfo();
            }
            catch (Exception ex)
            {
                Output = $"Method execution error: {ex.Message}";
            }
        }

        [RelayCommand]
        private void ApplyBoundingRectangle()
        {
            if (_currentShape == null) return;
            
            try
            {
                var type = _currentShape.GetType();
                
                if (type == typeof(Ellipse))
                {
                    double newRadiusX = (BoundingRectX2 - BoundingRectX1) / 2;
                    double newRadiusY = (BoundingRectY2 - BoundingRectY1) / 2;
                    double newCenterX = (BoundingRectX1 + BoundingRectX2) / 2;
                    double newCenterY = (BoundingRectY1 + BoundingRectY2) / 2;
                    
                    type.GetProperty("RadiusX")?.SetValue(_currentShape, newRadiusX);
                    type.GetProperty("RadiusY")?.SetValue(_currentShape, newRadiusY);
                    type.GetProperty("CenterX")?.SetValue(_currentShape, newCenterX);
                    type.GetProperty("CenterY")?.SetValue(_currentShape, newCenterY);
                }
                else if (type == typeof(Rectangle))
                {
                    double newWidth = BoundingRectX2 - BoundingRectX1;
                    double newHeight = BoundingRectY2 - BoundingRectY1;
                    double newCenterX = (BoundingRectX1 + BoundingRectX2) / 2;
                    double newCenterY = (BoundingRectY1 + BoundingRectY2) / 2;
                    
                    type.GetProperty("Width")?.SetValue(_currentShape, newWidth);
                    type.GetProperty("Height")?.SetValue(_currentShape, newHeight);
                    type.GetProperty("CenterX")?.SetValue(_currentShape, newCenterX);
                    type.GetProperty("CenterY")?.SetValue(_currentShape, newCenterY);
                }
                else if (type == typeof(Line))
                {
                    double newEndX = BoundingRectX2;
                    double newEndY = BoundingRectY2;
                    double newCenterX = (BoundingRectX1 + BoundingRectX2) / 2;
                    double newCenterY = (BoundingRectY1 + BoundingRectY2) / 2;
                    
                    type.GetProperty("EndX")?.SetValue(_currentShape, newEndX);
                    type.GetProperty("EndY")?.SetValue(_currentShape, newEndY);
                    type.GetProperty("CenterX")?.SetValue(_currentShape, newCenterX);
                    type.GetProperty("CenterY")?.SetValue(_currentShape, newCenterY);
                }
                else if (type == typeof(Point))
                {
                    double newCenterX = (BoundingRectX1 + BoundingRectX2) / 2;
                    double newCenterY = (BoundingRectY1 + BoundingRectY2) / 2;
                    
                    type.GetProperty("CenterX")?.SetValue(_currentShape, newCenterX);
                    type.GetProperty("CenterY")?.SetValue(_currentShape, newCenterY);
                }
                
                UpdateShapeInfo();
                Output = "Bounding rectangle updated successfully";
            }
            catch (Exception ex)
            {
                Output = $"Error updating bounding rectangle: {ex.Message}";
            }
        }

        private void SetSelectedType(Type? type)
        {
            if (SetProperty(ref _selectedType, type))
            {
                HasSelectedType = type != null;
                LoadMethods();
            }
        }

        private void SetSelectedMethod(MethodInfo? method)
        {
            if (SetProperty(ref _selectedMethod, method))
            {
                HasSelectedMethod = method != null;
                ShowBoundingRectEditor = method?.Name == "GetBoundingRectangle";
                
                if (ShowBoundingRectEditor && _currentShape != null)
                {
                    var rect = _currentShape.GetBoundingRectangle();
                    BoundingRectX1 = rect.x1;
                    BoundingRectY1 = rect.y1;
                    BoundingRectX2 = rect.x2;
                    BoundingRectY2 = rect.y2;
                }
                
                LoadParameters();
            }
        }

        private void LoadMethods()
        {
            try
            {
                ShapeMethods.Clear();
                if (_currentShape == null) return;

                var methods = _currentShape.GetType()
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => !m.IsSpecialName);

                foreach (var method in methods)
                    ShapeMethods.Add(method);
            }
            catch (Exception ex)
            {
                Output = $"Error loading methods: {ex.Message}";
            }
        }

        private void LoadParameters()
        {
            MethodParameters.Clear();
            if (SelectedMethod == null) return;

            var parameters = SelectedMethod.GetParameters();
            HasParameters = parameters.Length > 0;

            foreach (var param in parameters)
                MethodParameters.Add(new MethodParameterViewModel(param));
        }

        private void ClearAll()
        {
            ShapeTypes.Clear();
            ShapeMethods.Clear();
            MethodParameters.Clear();
            Output = string.Empty;
            CurrentShapeInfo = string.Empty;
            _currentShape = null;
        }
    }
}