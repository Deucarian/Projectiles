# Package Validation

Validation project: `C:\Repositories\Deucarian\Projectiles-TestProject`

Unity version: `6000.3.5f1`

Command:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.3.5f1\Editor\Unity.exe' -batchmode -projectPath C:\Repositories\Deucarian\Projectiles-TestProject -runTests -testPlatform EditMode -testResults C:\Repositories\Deucarian\Projectiles-TestProject\TestResults-EditMode.xml -logFile C:\Repositories\Deucarian\Projectiles-TestProject\Unity-Test-3.log
```

Phase 1L result with Unity `6000.3.5f1`:

- EditMode run 1: `21` passed, `0` failed.
- EditMode run 2: `21` passed, `0` failed.

The validation project consumes all Deucarian dependencies and Projectiles through local `file:` package references. No remote was invented and the package was not added to Package Registry.
