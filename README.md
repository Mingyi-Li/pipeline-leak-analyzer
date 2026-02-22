# Pipeline Leak Localization & Risk Assessment Tool

A WPF (.NET 8) desktop application that performs leak localization and risk scoring using distributed pipeline pressure sensor data.

## Features
- CSV-based pipeline sensor ingestion
- Segment-based pressure gradient analysis
- Flow mass-balance validation
- Weighted multi-signal risk scoring
- Leak localization with confidence metric
- MVVM architecture

## Architecture
- WPF + MVVM
- C#
- Clean separation of Models / Services / ViewModels
- Asynchronous processing
- Unit-tested core analysis logic

## Example Output
(Put screenshot later)

## Future Improvements
- SignalR real-time streaming
- gRPC distributed processing
- Blazor web client