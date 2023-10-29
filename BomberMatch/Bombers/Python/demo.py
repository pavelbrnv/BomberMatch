import random


# Класс должен называться 'Bomber'. Его экземпляр будет создаваться в адаптере.
# Класс обязательно должен содержать в себе методы 'get_name', 'set_rules' и 'go'.
class Bomber(object):

    # Коды действий
    NONE = 0
    UP = 1
    DOWN = 2
    LEFT = 3
    RIGHT = 4
    BOMB = 10

    def __init__(self):
        self.name = "Python demo bot"
        self.match_actions_number = None    # Полное количество ходов, после которого будет объявлена ничья.
        self.detonation_radius = None       # Радиус поражения бомбы. Значение 2 означает, что взорвутся две соседние с бомбой клетки.
        self.time_to_detonate = None        # Количество ходов, через которые будут взрываться поставленныен бомбы.

    def get_name(self) -> str:
        return self.name

    def set_rules(self, match_actions_number, detonation_radius, time_to_detonate):
        self.match_actions_number = match_actions_number
        self.detonation_radius = detonation_radius
        self.time_to_detonate = time_to_detonate

    def go(self, arena, bombers, available_moves) -> int:
        for row in range(len(arena)):
            for column in range(len(arena[row])):
                if arena[row][column] == -1:
                    # Стена. Останавливает взрывную волну. Сюда нельзя передвинуться.
                    None
                if arena[row][column] == 0:
                    # Пустое поле. Можно передвинуться.
                    None
                if arena[row][column] == 1:
                    # Поле с бомбой, которая взорвется на следующем ходу. Сюда нельзя передвинуться.
                    None
                if arena[row][column] > 1:
                    # Поле с бомбой, которая взорвется через указанное количество ходов. Сюда нельзя передвинуться.
                    None

        # Первая строка 'bombers' содержит координаты игрока на поле.
        # Последующие строки содержат координаты живых соперников на поле.
        me_row = bombers[0][0]
        me_column = bombers[0][1]

        # 'available_moves' содержит коды доступных перемещений.
        # Так, если в списке есть число 2, то это означает, что игрок может переместиться вниз.
        # В качестве примера выбираем любое перемещение из доступных.
        move_code = available_moves[random.randint(0, len(available_moves) - 1)]

        # Для постановки бомбы к коду перемещения нужно прибавить число 10.
        # В качестве примера с небольшой вероятностью устанавливаем бомбу.
        if random.randint(0, 10) > 8:
            move_code += Bomber.BOMB

        return move_code
