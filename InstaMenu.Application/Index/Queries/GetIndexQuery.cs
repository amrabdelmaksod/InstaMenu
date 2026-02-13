using InstaMenu.Application.Index.DTOs;
using MediatR;

namespace InstaMenu.Application.Index.Queries;

public sealed record GetIndexQuery : IRequest<IndexViewModel>;
