using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Util.Random;

namespace DevUtil.Aspects.Services.Remoting_MiddleWare.Util
{
    /// <summary>
    /// Random Node List: not yet thoroughly tested!
    /// </summary>
    public sealed class RandomNodeList
    {
        private sealed class RandomNode
        {
            public readonly string _data;
            public RandomNode _next;
            public RandomNode _previous;

            public RandomNode(string data)
            {
                _data = data;
                _next = null;
                _previous = null;
            }
        }

        private RandomNode _head;
        private RandomNode _tail;
        private readonly SemaphoreSlim _addDelLock;
        private readonly RandomHelper _random;
        private int _nodeCount;

        public RandomNodeList()
        {
            _head = null;
            _tail = null;
            _addDelLock = new SemaphoreSlim(1, 1);
            _random = new RandomHelper();
            _nodeCount = 0;
        }

        /// <summary>
        /// Adds a node with data if it doesn't exist already.
        /// </summary>
        /// <param name="data"></param>
        public async Task<bool> Add(string data)
        {
            if (data == null)
                return false;

            if (ListHasData(data))
                return false;

            var newNode = new RandomNode(data);
            await _addDelLock.WaitAsync();
            if (ListHasData(data))
            {
                _addDelLock.Release();
                return false;
            }

            if (_tail == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                _tail._next = newNode;
                _tail._next._previous = _tail;
                _tail = newNode;
            }

            _nodeCount++;
            _addDelLock.Release();
            return true;
        }

        /// <summary>
        /// Deletes the node with data if it exists.
        /// </summary>
        /// <param name="data"></param>
        public async Task Del(string data)
        {
            if (!ListHasData(data))
                return;

            await _addDelLock.WaitAsync();
            var processingNode = _head;
            do
            {
                if (processingNode._data.Equals(data))
                {
                    if (_head == processingNode && _tail == processingNode) // del only-node
                    {
                        _head = null;
                        _tail = null;
                    }
                    else if (_head == processingNode) // del head-node
                    {
                        _head = processingNode._next;
                        _head._previous = null;
                    }
                    else if (_tail == processingNode) // del tail-node
                    {
                        _tail = processingNode._previous;
                        _tail._next = null;
                    }
                    else
                    {
                        processingNode._previous._next = processingNode._next;
                        processingNode._next._previous = processingNode._previous;
                    }

                    _nodeCount--;
                    break;
                }

                processingNode = processingNode._next;
            } while (processingNode != null);

            _addDelLock.Release();
        }

        /// <summary>
        /// Get the data of a random node.
        /// </summary>
        public string GetRandom()
        {
            if (_head == null)
                return null;

            var doNextCounter = _random.GetInt(0, _nodeCount);

            var processingNode = _head;
            string data = null; // Backup.
            for (; doNextCounter >= 0; doNextCounter--)
            {
                if (processingNode != null)
                    data = processingNode._data;

                if (processingNode._next != null)
                    processingNode = processingNode._next;
            }

            return processingNode != null ? processingNode._data : data;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (_head != null)
            {
                var processingNode = _head;
                do
                {
                    sb.Append(processingNode._data + ",");
                    processingNode = processingNode._next;
                } while (processingNode != null);
            }

            return sb.ToString();
        }

        private bool ListHasData(string data)
        {
            if (_head == null)
                return false;

            var processingNode = _head;
            do
            {
                if (processingNode._data.Equals(data))
                    return true;

                processingNode = processingNode._next;
            } while (processingNode != null);

            return false;
        }
    }
}