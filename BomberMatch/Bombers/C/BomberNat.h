#ifndef _BOMBER_NAT_H_
#define _BOMBER_NAT_H_

#ifdef WIN32
#define DLL_EXTERN extern "C" __declspec(dllexport)
#else
#define DLL_EXTERN extern "C"
#endif

/// @brief Имя бота
DLL_EXTERN void Name(char* nameBot, unsigned int size);

/// @brief Задание параметров игры
/// @param[in] matchActionsNumber Максимальное число ходов в игре, после которого она будет завершена с ничейным результатом.
/// @param[in] detonationRadius Радиус поражения бомб. Бомба взорывает все (участников и другие бомбы) в клетках, отстоящих от нее по горизонтали и вертикали вплоть до этой величины.
/// @param[in] timeToDetonate Число ходов, через которые будут взрываться бомбы.
DLL_EXTERN void SetRules(int matchActionsNumber, int detonationRadius, int timeToDetonate);

/// @brief Ход 
/// @param[in] arena Двумерный массив, описывающий текущий вид поля боя. Является массивом по строкам,
/// т.е. если матрица такая
/// 1 2 3
/// 4 5 6 
/// 7 8 9
/// то массив будет вот такой - [1 2 3 4 5 6 7 8 9]
/// @param[in] bombers Позиции бомбометателей на поле. Таблица размерности Nx2, где N - количество игроков. Первое число - индекс строки на поле, второе - индекс столбца.
DLL_EXTERN int Go(int *arena, unsigned int sizeArenaLine, unsigned int sizeArenaCol, int *bombers, unsigned int sizeBombers, int *availableMoves, unsigned int sizeAvailableMoves);


#endif /* _BOMBER_NAT_H_ */