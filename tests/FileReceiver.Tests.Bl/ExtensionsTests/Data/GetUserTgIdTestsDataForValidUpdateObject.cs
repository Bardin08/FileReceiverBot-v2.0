using System.Collections;
using System.Collections.Generic;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;

namespace FileReceiver.Tests.Bl.ExtensionsTests.Data
{
    public class GetUserTgIdTestsDataForValidUpdateObject : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                1, new Update()
                {
                    Message = new Message()
                    {
                        From = new User()
                        {
                            Id = 1
                        }
                    }
                }
            };
            yield return new object[]
            {
                1, new Update()
                {
                    InlineQuery = new InlineQuery()
                    {
                        From = new User()
                        {
                            Id = 1
                        }
                    }
                }
            };
            yield return new object[]
            {
                1, new Update()
                {
                    ChosenInlineResult = new ChosenInlineResult()
                    {
                        From = new User()
                        {
                            Id = 1
                        }
                    }
                }
            };
            yield return new object[]
            {
                1, new Update()
                {
                    CallbackQuery = new CallbackQuery()
                    {
                        From = new User()
                        {
                            Id = 1
                        }
                    }
                }
            };
            yield return new object[]
            {
                1, new Update()
                {
                    EditedMessage = new Message()
                    {
                        From = new User()
                        {
                            Id = 1
                        }
                    }
                }
            };
            yield return new object[]
            {
                1, new Update()
                {
                    ChannelPost = new Message()
                    {
                        From = new User()
                        {
                            Id = 1
                        }
                    }
                }
            };
            yield return new object[]
            {
                1, new Update()
                {
                    EditedChannelPost = new Message()
                    {
                        From = new User()
                        {
                            Id = 1
                        }
                    }
                }
            };
            yield return new object[]
            {
                1, new Update()
                {
                    ShippingQuery = new ShippingQuery()
                    {
                        From = new User()
                        {
                            Id = 1
                        }
                    }
                }
            };
            yield return new object[]
            {
                1, new Update()
                {
                    PreCheckoutQuery = new PreCheckoutQuery()
                    {
                        From = new User()
                        {
                            Id = 1
                        }
                    }
                }
            };
            yield return new object[]
            {
                1, new Update()
                {
                    PollAnswer = new PollAnswer()
                    {
                        User = new User()
                        {
                            Id = 1
                        }
                    }
                }
            };
            yield return new object[]
            {
                1, new Update()
                {
                    MyChatMember = new ChatMemberUpdated()
                    {
                        From = new User()
                        {
                            Id = 1
                        }
                    }
                }
            };
            yield return new object[]
            {
                1, new Update()
                {
                    ChatMember = new ChatMemberUpdated()
                    {
                        From = new User()
                        {
                            Id = 1
                        }
                    }
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
