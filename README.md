# AelfAutomation


# Files

```shell

AelfAutomation
  |- src
      |- AelfAutomation
          |- Program.cs   // start up class
          |- appsettings.json // environment and account settings
          |- notable.txt  // notable special seed list notable.txt, one line one symbol
          |- uniq.txt     // uniq special seed list notable.txt, one line one symbol
          |- notable_result_xxxxx // notable add result file
          |- uniq_result_xxxxxxxx // uniq add result file
          
```

Account will decode from local keystore with password in `appsettings.json`

default keystore path is : `~/.local/share/aelf/keys`, you can change it in `appsettings.json`

# Usage - Add special seed

Run Program.cs in IDE, with parameters

- `args[0]`: command: `"SpecialSeed.add"`
- `args[1]`: sender
- `args[2]`: type of SpecialSeed `"uniq"` or `"notable"`
- `args[3]`: `[optional]` pages of data to **RESEND**, values joined with `"," `

```shell
AelfAutomation.dll SpecialSeed.add 2CpKfnoWTk69u6VySHMeuJvrX2hGrMw9pTyxcD4VM6Q28dJrhk uniq 2,4,5
```

output file name will like this:

`notable_result_2023_11_11_16_10_10`

`notable_result_2023_11_11_16_10_10`

output file content will like this:
```shell
# page, fromSymbol, toSymbol, transactionHash
2,ARO,FEO,90589bec79316e9d58aae131645eca4de9e12e301679e3cc5912c6c99e59f386
4,JET,MEL,aa03cb99e57d7fa293e3542e9fbe3cb305d6a3968be4d5d4678e95fd08f2d47a
5,MEO,REB,77d4fa0c0c2a21c762c6f02cfef098f54b312c256c08e1c73a03e60b5d461b71
```


# Usage - Check special seed
Run Program.cs in IDE, with parameters:

- `args[0]`: command: "SpecialSeed.check"
- `args[1]`: result fileName

```shell
AelfAutomation.dll SpecialSeed.check uniq_result_2023_11_11_16_10_10
```

console output result will like this:

```shell
# page, fromSymbol, toSymbol, transactionHash, status, error-info
2,ARO,FEO,90589bec79316e9d58aae131645eca4de9e12e301679e3cc5912c6c99e59f386 status=NOTEXISTED, error=
4,JET,MEL,aa03cb99e57d7fa293e3542e9fbe3cb305d6a3968be4d5d4678e95fd08f2d47a status=NOTEXISTED, error=
5,MEO,REB,77d4fa0c0c2a21c762c6f02cfef098f54b312c256c08e1c73a03e60b5d461b71 status=MINED, error=
Pending pages: 
Failed pages: 2,4
```
