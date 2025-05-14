using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MyList
{
    public class Node<T>
    {
        public T Value; // Значение узла
        public Node<T> Next; // Ссылка на следующий узел

        public Node(T value)
        {
            Value = value;
            Next = null;
        }
    }
    public class MyList<T> : IEnumerable<T>, ICollection<T>
    {

        private Node<T> head; // Указатель на первый узел
        private Node<T> tail; // Указатель на последний узел
        private int size; // Количество элементов в списке

        public MyList()
        {
            head = null; // Инициализация головы
            tail = null; // Инициализация хвоста
            size = 0; // Установка начального размера
        }
        /// <summary>
        /// Явная реализация необобщённого GetEnumerator()
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Обобщённая версия GetEnumerator()
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() => new ListEnumerator<T>(head, size);
        // Метод для добавления элемента в конец списка
        public void Add(T item)
        {
            Node<T> newNode = new Node<T>(item); // Создаем новый узел

            if (head == null) // Если список пуст
            {
                head = newNode; // Устанавливаем голову
                tail = newNode; // Устанавливаем хвост
            }
            else
            {
                tail.Next = newNode; // Присоединяем новый узел к хвосту
                tail = newNode; // Обновляем хвост
            }

            size++; // Увеличиваем размер списка
        }

        // Метод для вставки элемента в заданную позицию
        public void Insert(int index, T item)
        {
            if (index < 0 || index > size) // Проверка на допустимый индекс
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс вне допустимого диапазона.");
            }

            Node<T> newNode = new Node<T>(item); // Создаем новый узел

            if (index == 0) // Вставка в начало
            {
                newNode.Next = head; // Новый узел указывает на текущую голову
                head = newNode; // Новый узел становится головой
                if (tail == null) // Если список был пуст
                {
                    tail = newNode; // Устанавливаем хвост
                }
            }
            else
            {
                Node<T> current = head; // Начинаем с головы
                for (int i = 0; i < index - 1; i++) // Находим узел перед индексом
                {
                    current = current.Next;
                }

                newNode.Next = current.Next; // Новый узел указывает на следующий узел
                current.Next = newNode; // Узел перед индексом указывает на новый узел

                if (newNode.Next == null) // Если вставили в конец
                {
                    tail = newNode; // Обновляем хвост
                }
            }

            size++; // Увеличиваем размер списка
        }

        // Метод для удаления элемента по индексу
        public bool RemoveAt(int index)
        {
            if (index < 0 || index >= size) // Проверка на допустимый индекс
            {
                return false; // Индекс вне диапазона
            }

            if (index == 0) // Удаление из начала
            {
                head = head.Next; // Голова указывает на следующий узел
                if (head == null) // Если список стал пустым
                {
                    tail = null; // Сбрасываем хвост
                }
            }
            else
            {
                Node<T> current = head; // Начинаем с головы
                for (int i = 0; i < index - 1; i++) // Находим узел перед индексом
                {
                    current = current.Next;
                }

                current.Next = current.Next.Next; // Удаляем узел по индексу

                if (current.Next == null) // Если удалили последний узел
                {
                    tail = current; // Обновляем хвост
                }
            }

            size--; // Уменьшаем размер списка
            return true; // Успешное удаление
        }

        // Метод для получения элемента по индексу
        public T Get(int index)
        {
            if (index < 0 || index >= size) // Проверка на допустимый индекс
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс вне допустимого диапазона.");
            }

            Node<T> current = head; // Начинаем с головы
            for (int i = 0; i < index; i++) // Перемещаемся к нужному индексу
            {
                current = current.Next;
            }

            return current.Value; // Возвращаем значение узла
        }

        // Метод для очистки списка
        public void Clear()
        {
            head = null; // Устанавливаем голову в null
            tail = null; // Устанавливаем хвост в null
            size = 0; // Сбрасываем размер
        }

        // Свойство для получения текущего размера списка
        public int Size => size; // Свойство для получения размера

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            foreach (var item in collection)
            {
                AddLast(item); // Добавляем каждый элемент в конец списка
            }
        }

        private void AddLast(T item)
        {
            var newNode = new Node<T>(item);

            if (head == null)
            {
                head = newNode;
                tail = newNode;
            }
            else
            {
                tail.Next = newNode;
                tail = newNode;
            }

            size++;
        }        
        public T this[int index] //реализация индексации списка
        {
            get
            {
                return GetToIndex(index).Value;
            }
            set
            {
                var curr = GetToIndex(index);
                curr.Value = value;
            }
        }

        private Node<T> GetToIndex(int index)
        {
            if (index < 0  || index >= size)
                throw new IndexOutOfRangeException();

            var curr = head;
            for (int i = 0; i < index; i++)
                curr = curr.Next;

            return curr;
        }

        public bool IsReadOnly => false;

        public int Count => throw new NotImplementedException();

        public bool Contains(T item)
        {
            var curr = head;
            for (int i = 0; i < size; i++)
            {
                if (curr.Value.Equals(item))
                    return true;
            }
            return false;
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), "Массив не может быть null.");

            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Индекс вне границ массива.");

            if (array.Length - arrayIndex < size)
                throw new ArgumentException("В целевом массиве недостаточно места.");

            var curr = GetToIndex(arrayIndex);

            for (int i = arrayIndex, j = 0; i < size; i++, j++)
            {
                array[j] = curr.Value;
                curr = curr.Next;
            }
        }
        public bool Remove(T item)
        {
            var curr = head;
            for (int i = 0; i < size; i++)
            {
                if (curr.Value.Equals(item))
                {
                    this.RemoveAt(i);
                    return true;
                }
                curr = curr.Next;
            }
            return false;
        }


        /// <summary>
        /// Перечислитель списка
        /// </summary>
        /// <typeparam name="T"></typeparam>
        class ListEnumerator<T> : IEnumerator<T>
        {
            Node<T> Head;
            int Pointer = -1;
            int Count;
            Node<T> Curr;
            public ListEnumerator(Node<T> head, int count)
            {
                Head = Curr = head;
                Count = count;
            }
            public T Current
            {
                get
                {
                    if (Head == null  || Pointer == -1 || Pointer >= Count)
                        throw new InvalidOperationException();                   

                    return Curr.Value;
                }
            }
            object IEnumerator.Current => Current;
            public bool MoveNext()
            {
                Pointer++;
                if (Pointer >= Count)
                    return false;

                if (Pointer == 0)
                    Curr = Head;
                else
                    Curr = Curr?.Next;

                return Curr != null;
            }
            public void Reset()
            {
                Pointer = -1;
                Curr = null;
            }
            public void Dispose() { }
        }
    }
}
    
