@echo off
SET KSPExe="D:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program\KSP_x64.exe"  -popupwindow
SET KSPDir="D:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program\GameData"
robocopy "GameData" %KSPDir% /E
%KSPExe%
cls