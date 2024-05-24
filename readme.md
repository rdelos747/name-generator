# Name Generator

## Running 
```
dotnet run <command name> <arguments>
```
See `dotnet run` or `dotnet run help` for complete list of arguments and formats.

## Data sources
The name generator pulls data from the `source-data` directory. The lists there are compiled from data stored in the `raw-data` directory, either via manual copy and paste or help via some helper command, eg `dotnet run parse-raw-csv`. The `raw-data` directory mainly serves as a backup/ historical reference.

### Sources
https://github.com/zonination/emperors/blob/master/emperors.csv
https://github.com/whalesalad/hostnames/blob/master/greek-gods.txt
https://github.com/whalesalad/hostnames/blob/master/planets-moons.txt
