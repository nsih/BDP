using System;
namespace BS.Utils{
    public class Random
    {
        public static T RandomEnum<T>(){
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(new System.Random().Next(0, values.Length));
        }
    }
}
