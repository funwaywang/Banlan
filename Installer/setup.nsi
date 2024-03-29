Unicode true
!include x64.nsh
; Script generated by the HM NIS Edit Script Wizard
; 
; HM NIS Edit Wizard helper defines
!define PRODUCT_NAME "Banlan"
!define PRODUCT_FULLNAME "Banlan"
!define PRODUCT_VERSION "1.21.1216.0"
!define PRODUCT_PUBLISHER "Funway Wang"
!define PRODUCT_DEFSTARTMENU "Banlan"
!define PRODUCT_WEB_SITE "https://github.com/funwaywang/Banlan"
!define PRODUCT_DIR_REGKEY "Software\Banlan"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\Banlan"
!define PRODUCT_STARTMENU_REGVAL "NSIS:StartMenuDir"
!define MAINFILES_LOCATION "..\Src\bin\Release"
!define SWATCHES_LOCATION "..\Swatches"
!define APPLICATION_MUTEX "Banlan color swatch manage tool"

!define SHORTCUT_UNINSTALL "Uninstall"
!define SHORTCUT_HOMEPAGE "Home Page"

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "bin\Banlan_${PRODUCT_VERSION}.exe"
SetCompressor lzma
InstallDir "$PROGRAMFILES\${PRODUCT_FULLNAME}"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show
BrandingText "funwaywang@msn.com";

; MUI 1.67 compatible ------
!include "MUI.nsh"

; MUI Settings
!define MUI_ABORTWARNING
;!define MUI_ICON ".\Resources\Install.ico"
;!define MUI_UNICON ".\Resources\Install.ico"
;!define MUI_WELCOMEFINISHPAGE_BITMAP ".\Resources\win.bmp"
;!define MUI_UNWELCOMEFINISHPAGE_BITMAP ".\Resources\win.bmp"

; Language Selection Dialog Settings
!define MUI_LANGDLL_REGISTRY_ROOT "${PRODUCT_UNINST_ROOT_KEY}"
!define MUI_LANGDLL_REGISTRY_KEY "${PRODUCT_UNINST_KEY}"
!define MUI_LANGDLL_REGISTRY_VALUENAME "NSIS:Language"

; Welcome page
!define MUI_WELCOMEPAGE_TITLE "${PRODUCT_FULLNAME} ${PRODUCT_VERSION}"
!define MUI_WELCOMEPAGE_TEXT "$_CLICK"
!insertmacro MUI_PAGE_WELCOME

; License page
#!insertmacro MUI_PAGE_LICENSE ".\Include\Licence.txt"

; Directory page
!insertmacro MUI_PAGE_DIRECTORY

; Start menu page
var ICONS_GROUP
!define MUI_STARTMENUPAGE_NODISABLE
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "${PRODUCT_DEFSTARTMENU}"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "${PRODUCT_UNINST_ROOT_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "${PRODUCT_UNINST_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "${PRODUCT_STARTMENU_REGVAL}"
!insertmacro MUI_PAGE_STARTMENU Application $ICONS_GROUP

; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!define MUI_FINISHPAGE_RUN "$INSTDIR\Banlan.exe"
!define MUI_FINISHPAGE_RUN_TEXT "Run ${PRODUCT_NAME} ${PRODUCT_VERSION} "
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "SimpChinese"
!insertmacro MUI_LANGUAGE "TradChinese"

VIProductVersion ${PRODUCT_VERSION}
VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "${PRODUCT_NAME}"
;VIAddVersionKey /LANG=${LANG_ENGLISH} "Comments" ""
VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "${PRODUCT_PUBLISHER}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalTrademarks" "${PRODUCT_PUBLISHER}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "${PRODUCT_PUBLISHER}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "${PRODUCT_NAME}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "${PRODUCT_VERSION}"

; Reserve files
!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS

; MUI end ------

Function .onInit
;	!insertmacro MUI_LANGDLL_DISPLAY
	${If} ${RunningX64} 
	   	SetRegView 64
		StrCpy $INSTDIR "$PROGRAMFILES64\${PRODUCT_FULLNAME}"
	${EndIf}
FunctionEnd

Section "MainSection" SEC01
	SetOverwrite ifnewer

	SetOutPath "$INSTDIR"
	File "${MAINFILES_LOCATION}\Banlan.exe"
	File "${MAINFILES_LOCATION}\Banlan.exe.config"
	File "${MAINFILES_LOCATION}\Colourful.dll"
	File "${MAINFILES_LOCATION}\System.Drawing.Common.dll"
	File "${MAINFILES_LOCATION}\System.ValueTuple.dll"

	SetOutPath "$INSTDIR\Swatches"
	File "${SWATCHES_LOCATION}\*.*"
	SetOutPath "$INSTDIR\Swatches\Corbusier"
	File "${SWATCHES_LOCATION}\Corbusier\*.*"
	
	; Shortcuts
	;!insertmacro MUI_STARTMENU_WRITE_BEGIN Application
	CreateDirectory "$SMPROGRAMS\$ICONS_GROUP"
	CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\${PRODUCT_FULLNAME}.lnk" "$INSTDIR\Banlan.exe"
	;!insertmacro MUI_STARTMENU_WRITE_END
SectionEnd

Section -AdditionalIcons
	SetOutPath $INSTDIR
	!insertmacro MUI_STARTMENU_WRITE_BEGIN Application
	WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
	CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\${SHORTCUT_HOMEPAGE}.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
	CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\${SHORTCUT_UNINSTALL}.lnk" "$INSTDIR\UnInstaller.exe"
	!insertmacro MUI_STARTMENU_WRITE_END
SectionEnd

Section -Post
	SetOutPath $INSTDIR
	WriteUninstaller "$INSTDIR\UnInstaller.exe"
	WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR"
	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\UnInstaller.exe"
	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\Banlan.exe"
	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd

Section -OnFinish

SectionEnd

Function un.onUninstSuccess
	HideWindow
	MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) Successfully uninstalled."
FunctionEnd

Function un.onInit
	${If} ${RunningX64} 
	   	SetRegView 64
	${EndIf}

;	!insertmacro MUI_UNGETLANGUAGE
	MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to uninstall $(^Name) ?" IDYES +2
	Abort

	System::Call 'kernel32::OpenMutex(i 0x100000, b 0, t "${APPLICATION_MUTEX}") i .R0'
	IntCmp $R0 0 notRunning
	    System::Call 'kernel32::CloseHandle(i $R0)'
		    MessageBox MB_OK|MB_ICONEXCLAMATION "${PRODUCT_FULLNAME} is running, please close it before running the uninstaller." /SD IDOK
			    Abort
	notRunning:
FunctionEnd

Section Uninstall
	!insertmacro MUI_STARTMENU_GETFOLDER Application $ICONS_GROUP
	
	Delete "$INSTDIR\Swatches\Corbusier\*.*"
	Delete "$INSTDIR\Swatches\*.*"
	RMDir "$INSTDIR\Swatches\Corbusier"
	RMDir "$INSTDIR\Swatches"

	Delete "$INSTDIR\Banlan.exe"
	Delete "$INSTDIR\Banlan.exe.config"
	Delete "$INSTDIR\Colourful.dll"
	Delete "$INSTDIR\System.Drawing.Common.dll"
	Delete "$INSTDIR\System.ValueTuple.dll"	
	Delete "$INSTDIR\${PRODUCT_NAME}.url"
	Delete "$INSTDIR\UnInstaller.exe"	
	RMDir "$INSTDIR"

	Delete "$SMPROGRAMS\$ICONS_GROUP\${PRODUCT_FULLNAME}.lnk"
	Delete "$SMPROGRAMS\$ICONS_GROUP\${SHORTCUT_HOMEPAGE}.lnk"
	Delete "$SMPROGRAMS\$ICONS_GROUP\${SHORTCUT_UNINSTALL}.lnk"
	RMDir "$SMPROGRAMS\$ICONS_GROUP"

	DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
	DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
	SetAutoClose true
SectionEnd
