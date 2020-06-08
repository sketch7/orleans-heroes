# Todo

- Create management hero screen - create/edit
- Implement `PlayerHero` grain and remove HP from hero e.g. `{ hero: "khazix", hp: 100, name: "Chiko" }` - with SignalR send changes on `PlayerHero` not `Hero`
- UI Imp
  - Heroes:
    - add Hero thumb
  - Hero Details:
    - back button
    - ability icons


## State

```js
{
  heroCategories: [
    { key: "recommanded", title: "Most recommanded", heroes: ["aatrox", "talon"] },
    { key: "top-assassins", title: "Top played assassins", heroes: ["aatrox", "talon"] },
  ],
  heroes: [
    {key:"aatrox", name: "Aatrox", role: "assassin", popularity: 8, },
    { key:"talon", name: "Talon", role: "assassin", popularity: 4, abilities: [
      { name: "NOXIAN DIPLOMACY", damage: 150, damageType: "attackDamage" },
    ]
    },
    ...
  ],
  player: { id: "chiko#001", name: "Chiko", hero: "talon", hp: 800, maxHp: 1000 },
  team: [
    { id: "chiko#001" name: "Chiko", hero: "talon", hp: 800, maxHp: 1000 },
    { id: "chiko#001" name: "Chiko", hero: "talon", hp: 800, maxHp: 1000 },
    { id: "chiko#001" name: "Chiko", hero: "talon", hp: 800, maxHp: 1000 },
    { id: "chiko#001" name: "Chiko", hero: "talon", hp: 800, maxHp: 1000 },
  ],
}
```