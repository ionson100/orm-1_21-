namespace ORM_1_21_
{
    /// <summary>
    /// Тип генератора первичного ключа таблицы
    /// </summary>
    public enum Generator
    {
        /// <summary>
        /// автоинкрементный генератор в таблице
        /// </summary>
        Native = 0,
        /// <summary>
        /// Назначаемый пользователем
        /// </summary>
        Assigned
    }
}