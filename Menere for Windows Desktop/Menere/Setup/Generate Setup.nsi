!addplugindir ".\"

!include "MUI2.nsh"
!include "checkDotNetFull.nsh"
!include UAC.nsh


; The name of the installer
Name "Meneré"

; The file to write
OutFile "Setup-Menere.exe"



; The default installation directory
InstallDir "$PROGRAMFILES\Dog Food Soft\Menere\"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Dog Food Soft\Menere" "Install_Dir"

; Request application privileges for Windows Vista
; Back to user for mixed use
RequestExecutionLevel user

Function .onInit
uac_tryagain:
!insertmacro UAC_RunElevated
#MessageBox mb_TopMost "0=$0 1=$1 2=$2 3=$3"
${Switch} $0
${Case} 0
	${IfThen} $1 = 1 ${|} Quit ${|} ;we are the outer process, the inner process has done its work, we are done
	${IfThen} $3 <> 0 ${|} ${Break} ${|} ;we are admin, let the show go on
	${If} $1 = 3 ;RunAs completed successfully, but with a non-admin user
		MessageBox mb_IconExclamation|mb_TopMost|mb_SetForeground "This installer requires admin access, try again" /SD IDNO IDOK uac_tryagain IDNO 0
	${EndIf}
	;fall-through and die
${Case} 1223
	MessageBox mb_IconStop|mb_TopMost|mb_SetForeground "This installer requires admin privileges, aborting!"
	Quit
${Case} 1062
	MessageBox mb_IconStop|mb_TopMost|mb_SetForeground "Logon service not running, aborting!"
	Quit
${Default}
	MessageBox mb_IconStop|mb_TopMost|mb_SetForeground "Unable to elevate , error $0"
	Quit
${EndSwitch}
FunctionEnd


;--------------------------------

  !define MUI_ABORTWARNING



!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "logoSetupSmall.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP "logoSetupBig.bmp"
!define MUI_WELCOMEPAGE_TITLE "Meneré"
!define MUI_WELCOMEPAGE_TEXT "RSS feed reader client$\r$\n$\r$\nPlease stop any instance of Meneré prior to installing this version."
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "Dog Food Soft\Menere"
!define MUI_ICON "..\Images\MenereIconSetup.ico"
!define MUI_UNICON "uninstall.ico"


Var StartMenuFolder
; Pages

  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_LICENSE "License.txt"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY

  !define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
  !define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\Dog Food Soft\Menere" 
  !define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
  !insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder
  !insertmacro MUI_PAGE_INSTFILES
;  !define MUI_FINISHPAGE_RUN "Menere.exe"
  !define MUI_FINISHPAGE_RUN
  !define MUI_FINISHPAGE_RUN_FUNCTION FinishRun   
  !insertmacro MUI_PAGE_FINISH
  !insertmacro MUI_UNPAGE_WELCOME
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  !insertmacro MUI_UNPAGE_FINISH



Function FinishRun
!insertmacro UAC_AsUser_ExecShell "" "Menere.exe" "" "" ""
FunctionEnd

;--------------------------------




!insertmacro MUI_LANGUAGE "English"

; LoadLanguageFile "${NSISDIR}\Contrib\Language files\English.nlf"
;--------------------------------
;Version Information

  VIProductVersion "1.3.0.0"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "Menere"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "Dog Food Soft"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "© 2013 Dog Food Soft"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "RSS client"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "1.0.0"


Function un.UninstallDirs
    Exch $R0 ;input string
    Exch
    Exch $R1 ;maximum number of dirs to check for
    Push $R2
    Push $R3
    Push $R4
    Push $R5
       IfFileExists "$R0\*.*" 0 +2
       RMDir "$R0"
     StrCpy $R5 0
    top:
     StrCpy $R2 0
     StrLen $R4 $R0
    loop:
     IntOp $R2 $R2 + 1
      StrCpy $R3 $R0 1 -$R2
     StrCmp $R2 $R4 exit
     StrCmp $R3 "\" 0 loop
      StrCpy $R0 $R0 -$R2
       IfFileExists "$R0\*.*" 0 +2
       RMDir "$R0"
     IntOp $R5 $R5 + 1
     StrCmp $R5 $R1 exit top
    exit:
    Pop $R5
    Pop $R4
    Pop $R3
    Pop $R2
    Pop $R1
    Pop $R0
FunctionEnd




; The stuff to install
Section "Menere"

  SectionIn RO
  
  Call CheckAndInstallDotNet


;SetOutPath "$INSTDIR\\Images\\Artwork"
;  File "..\Images\Artwork\*"

  SetOutPath $INSTDIR

  ; Put file there
  File "Documentation.URL"
  File "..\Images\MenereIconSetup.ico"
  File "..\*.exe"
  File "..\*.dll"
  File "..\*.pdb"
  File "..\*.config"
  File "..\*.manifest"

  File "LICENSE.txt"
  File "Documentation.ico"


;  SetOutPath "$APPDATA\Dog Food Soft\Menere\themes"
 ; File /r "..\Themes\IncludedExamples\*"
;   SetOutPath $INSTDIR
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\Menere" "Install_Dir" "$INSTDIR"

  ; Enable WebBrowser not being quirks mode
  WriteRegDWORD HKCU "Software\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION" "Menere.exe" 8888
  WriteRegDWORD HKCU "Software\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION" "Menere.vshost.exe" 8888
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Menere" "DisplayName" "Menere"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Menere" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Menere" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Menere" "NoRepair" 1
  WriteUninstaller "uninstall.exe"

SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

!insertmacro MUI_STARTMENU_WRITE_BEGIN Application

  SetShellVarContext all
  CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
  CreateShortCut "$SMPROGRAMS\$StartMenuFolder\\Menere.lnk" "$INSTDIR\Menere.exe" "" "$INSTDIR\MenereIconSetup.ico" 0
  ;CreateShortCut "$SMPROGRAMS\$StartMenuFolder\\Documentation.lnk" "$INSTDIR\Documentation.URL" "" $INSTDIR\Documentation.ico" 0
  CreateShortCut "$SMPROGRAMS\$StartMenuFolder\\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  SetShellVarContext current
  
!insertmacro MUI_STARTMENU_WRITE_END 
SectionEnd


;--------------------------------

; Uninstaller

Section "Uninstall"

  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Dog Food Soft\Menere"
  DeleteRegKey HKLM "Software\Dog Food Soft\Menere"
  ; Remove files and uninstaller
  Delete $INSTDIR\*.*

  ; Remove shortcuts, if any
  !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder
    
	SetShellVarContext all
  Delete "$SMPROGRAMS\$StartMenuFolder\\*.*"
  SetShellVarContext current
  


  DeleteRegKey HKCU "Software\Dog Food Soft\Menere"


  ; Remove directories used
   ; RMDir "$SMPROGRAMS\$StartMenuFolder"
Push 10 #maximum amount of directories to remove
  Push "$SMPROGRAMS\$StartMenuFolder" #input string
    Call un.UninstallDirs

   
  ; RMDir "$INSTDIR"
  
  Push 10 #maximum amount of directories to remove
  Push $INSTDIR #input string
    Call un.UninstallDirs


SectionEnd
