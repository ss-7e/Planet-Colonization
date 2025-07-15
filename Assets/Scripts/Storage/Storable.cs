using UnityEngine;
public interface IStorable
{
    int Id { get; }
    int maxCount { get; }
    int currentCount { get; set; }
}
