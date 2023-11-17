// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using ResBenchmarks;

BenchmarkRunner.Run<ResVsOthers>();
BenchmarkRunner.Run<ResVsError>();
