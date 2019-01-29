; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define ApplicationName 'Blink'
#define ApplicationVersion GetFileVersion('..\Blink\BlinkClient\bin\Release\BlinkClient.exe')

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId                   ={{1004820E-4DA8-4B9E-B289-3D5FC9FB5C45}
AppName                 ={#ApplicationName}
AppVersion              ={#ApplicationVersion}
AppVerName              ={#ApplicationName} {#ApplicationVersion}
AppPublisher            =Jonathan B�caro
AppPublisherURL         =http://jonathanbucaro.com
Compression             =lzma
DefaultDirName          ={userappdata}\Blink
DisableDirPage          =yes
DisableWelcomePage      =no
DefaultGroupName        =Blink
DisableProgramGroupPage =yes
OutputBaseFilename      =Blink Setup
PrivilegesRequired      =lowest
SetupIconFile           =Resources\lightbulb.ico
SolidCompression        =yes
UninstallDisplayIcon    ={userappdata}\Blink\Blink.exe
VersionInfoVersion      ={#ApplicationVersion}
WizardImageFile         =Resources\settings.bmp
LicenseFile             =Resources\mit_license.txt

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\Source\BlinkClient\bin\Release\BlinkClient.exe";            DestDir: "{userappdata}\Blink"; Flags: replacesameversion
Source: "..\Source\BlinkClient\bin\Release\EPPlus.dll";                 DestDir: "{userappdata}\Blink"; Flags: replacesameversion
Source: "..\Source\BlinkClient\bin\Release\Newtonsoft.Json.dll";        DestDir: "{userappdata}\Blink"; Flags: replacesameversion
Source: "..\Source\BlinkClient\bin\Release\branch.settings.json";       DestDir: "{userappdata}\Blink"; Flags: replacesameversion
Source: "..\Source\BlinkClient\bin\Release\BlinkLib.dll";               DestDir: "{userappdata}\Blink"; Flags: replacesameversion
Source: "Resources\folder.ico";                                         DestDir: "{userappdata}\Blink"; Flags: ignoreversion
Source: "Resources\spreadsheet.ico";                                    DestDir: "{userappdata}\Blink"; Flags: ignoreversion
Source: "Resources\lightbulb.ico";                                      DestDir: "{userappdata}\Blink"; Flags: ignoreversion
Source: "Resources\delete.ico";                                         DestDir: "{userappdata}\Blink"; Flags: ignoreversion

[Icons] 
Name: "{group}\Blink"; Filename: "{userappdata}\Blink\BlinkClient.exe"

[Registry]
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\BlinkFolder";                                                                 ValueData: "Blink folder structure";                                      Flags: deletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\BlinkSpreadsheet";                                                            ValueData: "Blink spreadsheet file";                                      Flags: deletekey

Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink";                    ValueType: expandsz; ValueName: "MUIVerb";                                                                                   Flags: uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink";                    ValueType: expandsz; ValueName: "SubCommands";                                                                               Flags: uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink";                    ValueType: string ;  ValueName: "Icon";            ValueData: """{userappdata}\Blink\lightbulb.ico""";                       Flags: uninsdeletekey

Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink\shell\cmd1";         ValueType: dword;    ValueName: "AttributeMask";   ValueData: "$1";                                                          Flags: uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink\shell\cmd1";         ValueType: dword;    ValueName: "AttributeValue";  ValueData: "$1";                                                          Flags: uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink\shell\cmd1";         ValueType: string ;  ValueName: "Icon";            ValueData: """{userappdata}\Blink\folder.ico""";                          Flags: uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink\shell\cmd1";         ValueType: string ;  ValueName: "MUIVerb";         ValueData: "Build folder structure";                                      Flags: uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink\shell\cmd1\command"; ValueType: string ;                                ValueData: """{userappdata}\Blink\BlinkClient.exe"" -structure ""%1""";   Flags: uninsdeletekey

Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink\shell\cmd2";         ValueType: dword;    ValueName: "AttributeMask";   ValueData: "$1";                                                          Flags: uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink\shell\cmd2";         ValueType: dword;    ValueName: "AttributeValue";  ValueData: "$1";                                                          Flags: uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink\shell\cmd2";         ValueType: string ;  ValueName: "Icon";            ValueData: """{userappdata}\Blink\spreadsheet.ico""";                     Flags: uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink\shell\cmd2";         ValueType: string ;  ValueName: "MUIVerb";         ValueData: "Generate spreadsheet file";                                   Flags: uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink\shell\cmd2\command"; ValueType: string ;                                ValueData: """{userappdata}\Blink\BlinkClient.exe"" -spreadsheet ""%1"""; Flags: uninsdeletekey

Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink\shell\cmd3";         ValueType: dword;    ValueName: "AttributeMask";   ValueData: "$1";                                                          Flags: uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink\shell\cmd3";         ValueType: dword;    ValueName: "AttributeValue";  ValueData: "$1";                                                          Flags: uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink\shell\cmd3";         ValueType: string ;  ValueName: "Icon";            ValueData: """{userappdata}\Blink\delete.ico""";                          Flags: uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink\shell\cmd3";         ValueType: string ;  ValueName: "MUIVerb";         ValueData: "Cleanse empty folders";                                       Flags: uninsdeletekey
Root: "HKCU"; Subkey: "Software\Classes\Directory\shell\Blink\shell\cmd3\command"; ValueType: string ;                                ValueData: """{userappdata}\Blink\BlinkClient.exe"" -cleanse ""%1""";     Flags: uninsdeletekey

[Code]

procedure InitializeWizard();
begin
  { Welcome page }
  { Hide the labels }
  WizardForm.WelcomeLabel1.Visible := False;
  WizardForm.WelcomeLabel2.Visible := False;
  { Stretch image over whole page }
  WizardForm.WizardBitmapImage.Width := WizardForm.WizardBitmapImage.Parent.Width;
  WizardForm.WizardBitmapImage.Height :=WizardForm.WizardBitmapImage.Parent.Height;

  { Finished page }
  { Hide the labels }
  WizardForm.FinishedLabel.Visible := False;
  WizardForm.FinishedHeadingLabel.Visible := False;
  { Stretch image over whole page }
  WizardForm.WizardBitmapImage2.Width := WizardForm.WizardBitmapImage2.Parent.Width;
  WizardForm.WizardBitmapImage2.Height := WizardForm.WizardBitmapImage2.Parent.Height;
end;