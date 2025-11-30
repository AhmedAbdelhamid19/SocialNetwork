namespace API.Interfaces;

public interface IUnitOfWork
{
    IMemberRepository MemberRepository { get; }
    IMessageRepository MessageRepository { get; }
    IFollowRepository FollowRepository { get; }
    Task<bool> Complete();
    bool HasChanges();
}
