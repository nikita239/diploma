using System;
using System.Collections.Generic;

namespace WpfApplication1.Models
{
    class NumbersDTO
    {
        public double H { get; set; }
        public double T1 { get; set; }
        public double T2 { get; set; }
    }
    public class MapValues
    {
        public string Solid { get; set; }
        public List<Map> Values { get; set; }
    }

    public class Map
    {
        public IntergralTemperatureParameters Name { get; set; }
        public double Value { get; set; }
    }

    public class DataStructure
    {
        public object TextBlock { get; set; }
        public string TextBlock2 { get; set; }
        public double? Value { get; set; }
        public string Units { get; set; }
        public string Description { get; set; }
    }

    public enum LevelingParameters
    {
        L, T, M, V
    }
    public enum PermafrostParameters
    {
        f, P, Р
    }
    public enum SeasonalFreezingParameters
    {
        γ, P, R, f, z
    }
    public enum IntergralTemperatureParameters
    {
        α, a, Δt, z
    }
}
