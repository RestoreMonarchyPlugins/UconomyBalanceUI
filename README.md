# Uconomy Balance UI
Simple UI that displays the player's Uconomy balance in the bottom center of the screen.

## Features
* Customizable with rich text formatting
* Animation when player earn or spend money
* Hides automatically to avoid conflicts with Gas Mask UI
* Hides automatically in death screen
* Matches the style of the vanilla UI
* Players can enable or disable the UI with a command

## Works with
* [Uconomy](https://restoremonarchy.com/servers/plugins/uconomy)
* [Uconomy XP](https://restoremonarchy.com/servers/plugins/uconomyxp)
* [Uconomy To Advanced Economy](https://restoremonarchy.com/servers/plugins/uconomytoadvancedeconomy)

## Workshop
[Uconomy Balance UI](https://steamcommunity.com/sharedfiles/filedetails/?id=3441984831) - `3441984831`

## Commands
* **/balanceui** - Enable or disable the balance UI

## Permissions
```xml
<Permission Cooldown="0">balanceui</Permission>
```

## Configuration
```xml
<?xml version="1.0" encoding="utf-8"?>
<UconomyBalanceUIConfiguration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <MessageColor>yellow</MessageColor>
  <MessageIconUrl>https://i.imgur.com/XF9jnxo.png</MessageIconUrl>
  <EffectId>27520</EffectId>
  <BalanceFormat>[[b]]$ {0}[[/b]]</BalanceFormat>
  <EarnMoneyFormat>[[color=#3e9c35]]+${0}[[/color]]</EarnMoneyFormat>
  <SpendMoneyFormat>[[color=#ff0000]]-${0}[[/color]]</SpendMoneyFormat>
  <ShowUIEffectByDefault>true</ShowUIEffectByDefault>
  <JsonFilePath>{rocket_directory}/Plugins/UconomyBalanceUI/PlayerPreferences.json</JsonFilePath>
</UconomyBalanceUIConfiguration>
```

## Translations
```xml
<?xml version="1.0" encoding="utf-8"?>
<Translations xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Translation Id="BalanceUIDisabled" Value="Balance UI has been disabled" />
  <Translation Id="BalanceUIEnabled" Value="Balance UI has been enabled" />
</Translations>
```