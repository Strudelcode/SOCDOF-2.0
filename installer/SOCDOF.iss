#define AppName "SOCDOF 2.0"
#define AppVersion "2.0.2"
#define AppPublisher "Yuri / Strudel"
#define AppExeName "SOCDOF_2.0.exe"
#define SourceDir "..\publish"
#define OutputDir "..\installer-output"

[Setup]
AppId={{B8CBE0F7-5E7D-4B69-9A4A-70F079B5B0C4}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\SOCDOF 2.0
DisableDirPage=no
DefaultGroupName={#AppName}
OutputDir={#OutputDir}
OutputBaseFilename=SOCDOF_setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
Uninstallable=yes
ArchitecturesInstallIn64BitMode=x64
DisableProgramGroupPage=yes

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"

[Files]
Source: "{#SourceDir}\{#AppExeName}"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon
