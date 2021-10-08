Razensoft.Mediator
======================================================

Simple mediator library for Unity. This library is inspired by MediatR in many ways but it adds a separation of input/output handling and sync/async requests to make it more suitable for gamedev needs.

## Installation

There are several ways to install this library into our project:

- **Plain install**: Clone or [download](https://github.com/Razenpok/Razensoft.Mediator/archive/master.zip) this repository and put it somewhere in your Unity project
- **Unity Package Manager (UPM)**: Add the following line to *Packages/manifest.json*:
  - `"com.razensoft.mediator": "https://github.com/Razenpok/Razensoft.Mediator.git?path=src/Razensoft.Mediator#1.0.0",`
- **[OpenUPM](https://openupm.com)**: After installing [openupm-cli](https://github.com/openupm/openupm-cli), run the following command:
  - `openupm add com.razensoft.mediator`

## Usage

```csharp
public class PurchaseArmorUpgradeButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private int _id;

    private void Start()
    {
        _button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        var isSuccess = InputMediator.Send(new Command{ Id = _id });
        if (isSuccess)
        {
            SpawnSomeFancyParticlesOrWhatever();
        }
        else
        {
            ShowNotEnoughMoneyMessage();
        }
    }

    public class Command : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class CommandHandler : InputRequestHandler<Command, bool>
    {
        private readonly Player _player;
        private readonly Analytics _analytics;

        public CommandHandler(Player player, Analytics analytics)
        {
            _player = player;
            _analytics = analytics;
        }

        protected override bool Handle(Command command)
        {
            _analytics.PurchaseArmorUpgradeClicked();
            if (!_player.CanPurchaseArmorUpgrade(command.Id))
            {
                return false;
            }

            _player.PurchaseArmorUpgrade(command.Id);
            _analytics.PurchaseArmorUpgradeBought();
            return true;
        }
    }
}
```

## Readings and watchings

- [Clean Architecture by Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Vertical Slices by Jimmy Bogard](https://jimmybogard.com/vertical-slice-architecture/), and [his talk about it at NDC Sydney 2018](https://www.youtube.com/watch?v=SUiWfhAhgQw)
