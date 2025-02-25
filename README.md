# Unity.Wait

WaitForSeconds but without alloc

## Installation

Add the dependency to your `manifest.json`

```json
{
  "dependencies": {
    "jd.boiv.in.wait": "https://github.com/starburst997/Unity.Wait.git"
  }
}
```

## Usage

```csharp
yield return Wait.Seconds(0.2f);
```