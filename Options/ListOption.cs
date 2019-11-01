namespace Kesco.Lib.Win.Data.Options
{
    public enum ListOptionModes
    {
        AND,
        OR
    }

    public abstract class ListOption : Option
    {
        private ListOptionModes mode;
        private int minCount; // �� ��������� � ������ ������ ���� ��������
        private int maxCount; // �� ����� �� ����������
        private int shortTextMaxCount; // ���� �� ������ 2 �� � shortText ��������� 1, 2 � ��.

        #region ACCESSORS

        protected int MinCount
        {
            get { return minCount; }
            set { minCount = value; }
        }

        protected int MaxCount
        {
            get { return maxCount; }
            set { maxCount = value; }
        }

        protected int ShortTextMaxCount
        {
            get { return shortTextMaxCount; }
            set { shortTextMaxCount = value; }
        }

        public ListOptionModes Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        #endregion

        public ListOption(string name)
            : base(name)
        {
        }

        public override void Init()
        {
            base.Init();
            mode = ListOptionModes.OR;
            minCount = 1;
            maxCount = -1;
            shortTextMaxCount = 2;
        }
    }
}