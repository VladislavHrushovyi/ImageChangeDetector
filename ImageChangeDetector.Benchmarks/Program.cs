// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using ImageChangeDetector.Benchmarks;

var summary = BenchmarkRunner.Run<ImageChangeDetectorBenchmark>();

Console.ReadKey();