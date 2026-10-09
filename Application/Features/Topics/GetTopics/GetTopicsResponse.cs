using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Topics.GetTopics;

public record GetTopicsResponse(IReadOnlyList<TopicItem> Topics);

public record TopicItem(int Id, string Name);
