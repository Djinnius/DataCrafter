using DataCrafter.Entities;

namespace DataCrafter.Services.ConsoleWriters;
internal interface IGeneratedDataConsoleWriter
{
    void PrintDynamicClassToConsole(List<DynamicClass> dynamicClasses);
    void PrintDynamicClassSamplesToConsole(List<DynamicClass> dynamicClasses);
}
