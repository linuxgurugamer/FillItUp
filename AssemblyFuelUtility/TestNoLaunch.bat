@echo off
SET KSPDir="D:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program\GameData"
robocopy "GameData" %KSPDir% /E
cls