namespace Sharpen
{
    public class CyclicBarrier
    {
        private readonly CountDownLatch counter;

        public CyclicBarrier (int parties)
        {
            this.counter = new CountDownLatch (parties);
        }

        public void Await ()
        {
            this.counter.CountDown ();
            this.counter.Await ();
        }
    }
}
