namespace Shared
{
    public class TableColumn<TItem>
    {
        // Название колонки, которое будет в заголовке
        public string Label { get; set; }

        // Функция-селектор для получения значения ячейки из объекта данных
        // Возвращает object, чтобы можно было рендерить разные типы данных (строки, числа, и т.д.)
        public Func<TItem, object> GetValue { get; set; }
    }
}
