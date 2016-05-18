using System.Threading.Tasks;
using System;
#if NHVersion7
using NServiceBus.Sagas;
#else
using NServiceBus.Saga;
#endif

using NServiceBus;

namespace Shared
{
    class TestSaga : Saga<TestSagaData>, IAmStartedByMessages<TMsg>
    {
#if NHVersion7 || NHVersion6
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TestSagaData> mapper)
#else
    public override void ConfigureHowToFindSaga()
#endif
        {
            
        }

#if NHVersion7
        public Task Handle(TMsg message, IMessageHandlerContext context)
        {
            return Task.FromResult(0);
        }
#else
        public void Handle(TMsg message)
        {
            throw new NotImplementedException();
        }
#endif
    }

    class TestSagaWithList : Saga<TestSagaDataWithList>, IAmStartedByMessages<TMsg>
    {
#if NHVersion7 || NHVersion6
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TestSagaDataWithList> mapper)
#else
        public override void ConfigureHowToFindSaga()
#endif
        {

        }

#if NHVersion7
        public Task Handle(TMsg message, IMessageHandlerContext context)
        {
            return Task.FromResult(0);
        }
#else
        public void Handle(TMsg message)
        {
            throw new NotImplementedException();
        }
#endif
    }

    class TestSagaWithComposite : Saga<TestSagaDataWithComposite>, IAmStartedByMessages<TMsg>
    {
#if NHVersion7 || NHVersion6
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TestSagaDataWithComposite> mapper)
#else
        public override void ConfigureHowToFindSaga()
#endif
        {
        }

#if NHVersion7
        public Task Handle(TMsg message, IMessageHandlerContext context)
        {
            return Task.FromResult(0);
        }
#else
        public void Handle(TMsg message)
        {
            throw new NotImplementedException();
        }
#endif
    }
    public  class TMsg: IMessage
    {
    }
}
