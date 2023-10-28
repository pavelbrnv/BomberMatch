import random


class Point(object):
    def __init__(self, row, column):
        self.row = row
        self.column = column

    def is_close(self, other, radius) -> bool:
        return abs(self.row - other.row) <= radius and abs(self.column - other.column) <= radius

    def get_row_distance(self, other) -> int:
        return self.row - other.row

    def get_column_distance(self, other) -> int:
        return self.column - other.column


class Bomber(object):
    NONE = 0
    UP = 1
    DOWN = 2
    LEFT = 3
    RIGHT = 4
    BOMB = 10

    def __init__(self):
        self.name = "Cobra"
        self.match_actions_number = None
        self.detonation_radius = None
        self.time_to_detonate = None

    def get_name(self) -> str:
        return self.name

    def set_rules(self, match_actions_number, detonation_radius, time_to_detonate):
        self.match_actions_number = match_actions_number
        self.detonation_radius = detonation_radius
        self.time_to_detonate = time_to_detonate

    def go(self, arena, bombers, available_moves) -> int:
        if len(bombers) < 2:
            return available_moves[0]

        me = Point(bombers[0][0], bombers[0][1])
        enemy = Point(bombers[1][0], bombers[1][1])

        plant_bomb = False

        moves_from_danger = {Bomber.UP, Bomber.DOWN, Bomber.LEFT, Bomber.RIGHT}

        if Bomber.is_danger_up(arena, me, self.detonation_radius):
            moves_from_danger.remove(Bomber.UP)
        if Bomber.is_danger_down(arena, me, self.detonation_radius):
            moves_from_danger.remove(Bomber.DOWN)
        if Bomber.is_danger_left(arena, me, self.detonation_radius):
            moves_from_danger.remove(Bomber.LEFT)
        if Bomber.is_danger_right(arena, me, self.detonation_radius):
            moves_from_danger.remove(Bomber.RIGHT)

        if len(moves_from_danger) != 4:
            # go away from danger!
            moves_from_danger = moves_from_danger.intersection(available_moves)
            if len(moves_from_danger) == 0:
                move_code = Bomber.get_random_move_code(available_moves)
            else:
                move_code = Bomber.get_random_move_code(list(moves_from_danger))
        else:
            # chase him!
            move_code = Bomber.get_move_closer_code(me, enemy, available_moves)
            plant_bomb = me.is_close(enemy, self.detonation_radius + 1) and random.choice([True, False])

        if move_code not in available_moves:
            move_code = Bomber.get_random_move_code(available_moves)

        if plant_bomb:
            move_code += Bomber.BOMB

        return move_code

    @staticmethod
    def get_random_move_code(moves) -> int:
        move_index = random.randint(0, len(moves) - 1)
        return moves[move_index]

    @staticmethod
    def get_move_closer_code(me, other, available_moves) -> int:
        row_distance = me.get_row_distance(other)
        column_distance = me.get_column_distance(other)

        moves = list()

        if row_distance > 0 and Bomber.UP in available_moves:
            moves.append(Bomber.UP)
        elif row_distance < 0 and Bomber.DOWN in available_moves:
            moves.append(Bomber.DOWN)

        if column_distance > 0 and Bomber.LEFT in available_moves:
            moves.append(Bomber.LEFT)
        elif column_distance < 0 and Bomber.RIGHT in available_moves:
            moves.append(Bomber.RIGHT)

        if len(moves) == 0:
            moves.extend(available_moves)

        return Bomber.get_random_move_code(moves)

    @staticmethod
    def is_bomb(field_value) -> bool:
        return field_value > 0

    @staticmethod
    def is_bomb_left(arena, point: Point, radius) -> bool:
        for i in range(radius + 1):
            column = point.column - i
            if column >= 0 and Bomber.is_bomb(arena[point.row][column]):
                return True
        return False

    @staticmethod
    def is_bomb_right(arena, point: Point, radius) -> bool:
        for i in range(radius + 1):
            column = point.column + i
            if column < len(arena[point.row]) and Bomber.is_bomb(arena[point.row][column]):
                return True
        return False

    @staticmethod
    def is_bomb_up(arena, point: Point, radius) -> bool:
        for i in range(radius + 1):
            row = point.row - i
            if row >= 0 and Bomber.is_bomb(arena[row][point.column]):
                return True
        return False

    @staticmethod
    def is_bomb_down(arena, point: Point, radius) -> bool:
        for i in range(radius + 1):
            row = point.row + i
            if row < len(arena) and Bomber.is_bomb(arena[row][point.column]):
                return True
        return False

    @staticmethod
    def is_bomb_left_right(arena, point: Point, radius) -> bool:
        return Bomber.is_bomb_left(arena, point, radius) or Bomber.is_bomb_right(arena, point, radius)

    @staticmethod
    def is_bomb_up_down(arena, point: Point, radius) -> bool:
        return Bomber.is_bomb_up(arena, point, radius) or Bomber.is_bomb_down(arena, point, radius)

    @staticmethod
    def is_danger_left(arena, point: Point, radius) -> bool:
        if Bomber.is_bomb_left(arena, point, radius + 1):
            return True
        left = Point(point.row, point.column - 1)
        return left.column >= 0 and Bomber.is_bomb_up_down(arena, left, radius)

    @staticmethod
    def is_danger_right(arena, point: Point, radius) -> bool:
        if Bomber.is_bomb_right(arena, point, radius + 1):
            return True
        right = Point(point.row, point.column + 1)
        return right.column < len(arena[point.row]) and Bomber.is_bomb_up_down(arena, right, radius)

    @staticmethod
    def is_danger_up(arena, point: Point, radius) -> bool:
        if Bomber.is_bomb_up(arena, point, radius + 1):
            return True
        up = Point(point.row - 1, point.column)
        return up.row >= 0 and Bomber.is_bomb_left_right(arena, up, radius)

    @staticmethod
    def is_danger_down(arena, point: Point, radius) -> bool:
        if Bomber.is_bomb_down(arena, point, radius + 1):
            return True
        down = Point(point.row + 1, point.column)
        return down.row < len(arena) and Bomber.is_bomb_left_right(arena, down, radius)
