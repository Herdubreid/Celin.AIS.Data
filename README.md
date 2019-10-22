# Celin.AIS.Data

An AIS Data Parser build with [Pidgin](https://github.com/benjamin-hodgson/Pidgin).


## Usage

For statement syntax reference, see [AIS Data Browser reference](https://herdubreid.github.io/aisDataBrowser/query-syntax.html).

### Code Example
Add Pidgin using statement and then call the `DataRequest` parser with the statement:
```csharp
using System;
using System.Text.Json;
using Pidgin;

namespace Celin
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Command  followed by Enter (exit with a blank line): ");
            Console.Write(">");
            var cmd = Console.ReadLine();
            while (cmd.Length > 0)
            {
                try
                {
                    var p = AIS.Data.DataRequest.Parser.ParseOrThrow(cmd);
                    var json = JsonSerializer.Serialize(p, new JsonSerializerOptions
                    {
                        IgnoreNullValues = true,
                        WriteIndented = true,
                        Converters =
                        {
                            new AIS.ActionJsonConverter(),
                            new AIS.GridActionJsonConverter()
                        }
                    });
                    Console.WriteLine(json);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed!\n{0}", e.Message);
                }
                Console.Write(">");
                cmd = Console.ReadLine();
            }
        }
    }
}
```
### Output Example
The `"f0101 (an8,alph) all(at1=C)"` statement will output:
```json
{
  "targetName": "F0101",
  "targetType": "table",
  "dataServiceType": "BROWSE",
  "findOnEntry": "TRUE",
  "returnControlIDs": "AN8|ALPH",
  "query": {
    "condition": [
      {
        "value": [
          {
            "content": "C",
            "specialValueId": "LITERAL"
          }
        ],
        "controlId": "AT1",
        "operator": "EQUAL"
      }
    ],
    "matchType": "MATCH_ALL"
  },
  "outputType": "GRID_DATA"
}
```
