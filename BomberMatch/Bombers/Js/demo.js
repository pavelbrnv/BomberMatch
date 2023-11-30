// Скрипт обязательно должен содержать функции getName, go, setRules
// Скрипт не должен импортировать или экспортировать модули

function getName() {
    return DemoBomber.name;
}

function go(arena, bombers, availableMoves) {
    return DemoBomber.go(arena, bombers, availableMoves);
}

function setRules(matchActionsNumber, detonationRadius, timeToDetonate) {
    DemoBomber.setRules(matchActionsNumber, detonationRadius, timeToDetonate);
}

class DemoBomber
{
    static name = "Js demo bot";

    static _matchActionsNumber; // Полное количество ходов, после которого будет объявлена ничья.
    static _detonationRadius; // Радиус поражения бомбы. Значение 2 означает, что взорвутся две соседние с бомбой клетки.
    static _timeToDetonate; // Количество ходов, через которые будут взрываться поставленныен бомбы.

    static go(arena, bombers, availableMoves) {
        for (let row = 0; row < arena.length; row++) {
            for (let column = 0; column < arena.length; column++) {
                let value = arena[row][column];
                if (value === -1) {
                    // Стена. Останавливает взрывную волну. Сюда нельзя передвинуться.
                    continue;
                }
                if (value === 0) {
                    // Пустое поле. Можно передвинуться.
                    continue;
                }
                if (value === 1) {
                    // Поле с бомбой, которая взорвется на следующем ходу. Сюда нельзя передвинуться.
                    continue;
                }
                if (value > 1) {
                    // Поле с бомбой, которая взорвется через указанное количество ходов. Сюда нельзя передвинуться.
                    continue;
                }
            }
        }

        // Первая строка 'bombers' содержит координаты игрока на поле.
        // Последующие строки содержат координаты живых соперников на поле.
        let meRow = bombers[0][0];
        let meColumn = bombers[0][1];

        // 'availableMoves' содержит коды доступных перемещений.
        // Так, если в списке есть число 2, то это означает, что игрок может переместиться вниз.
        // В качестве примера выбираем любое перемещение из доступных.
        let moveCode = availableMoves[Math.floor(Math.random() * availableMoves.length)];

        // Для постановки бомбы к коду перемещения нужно прибавить число 10.
        // В качестве примера с небольшой вероятностью устанавливаем бомбу.
        if (Math.random() > 0.8) {
            moveCode += action.bomb;
        }

        return moveCode;
    }

    static setRules(matchActionsNumber, detonationRadius, timeToDetonate) {
        this._matchActionsNumber = matchActionsNumber;
        this._detonationRadius = detonationRadius;
        this._timeToDetonate = timeToDetonate;
    }
}

// Коды действий
const action = {
    none: 0,
    up: 1,
    down: 2,
    left: 3,
    right: 4,
    bomb: 10,
}
