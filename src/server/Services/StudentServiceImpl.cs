using Grpc.Core;
using System.Collections.Concurrent;
using University;

namespace Server.Services
{
    public class StudentServiceImpl : StudentService.StudentServiceBase
    {
        public static readonly ConcurrentDictionary<string, Student> Students = new();

        public override Task<ListStudentsResponse> ListStudents(ListStudentsRequest request, ServerCallContext context)
        {
            var response = new ListStudentsResponse();
            response.Students.AddRange(Students.Values);
            return Task.FromResult(response);
        }

        public override Task<GetStudentResponse> GetStudent(GetStudentRequest request, ServerCallContext context)
        {
            if (!Students.TryGetValue(request.Id, out var student))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Student not found"));
            }
            return Task.FromResult(new GetStudentResponse { Student = student });
        }

        public override Task<CreateStudentResponse> CreateStudent(CreateStudentRequest request, ServerCallContext context)
        {
            var student = request.Student;
            if (string.IsNullOrWhiteSpace(student.Id))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Student Id is required"));
            }
            if (Students.ContainsKey(student.Id))
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, "Student already exists"));
            }
            Students[student.Id] = student;
            return Task.FromResult(new CreateStudentResponse { Student = student });
        }

        public override Task<UpdateStudentResponse> UpdateStudent(UpdateStudentRequest request, ServerCallContext context)
        {
            var student = request.Student;
            if (string.IsNullOrWhiteSpace(student.Id))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Student Id is required"));
            }
            if (!Students.ContainsKey(student.Id))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Student not found"));
            }
            Students[student.Id] = student;
            return Task.FromResult(new UpdateStudentResponse { Student = student });
        }

        public override Task<DeleteStudentResponse> DeleteStudent(DeleteStudentRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Id))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Student Id is required"));
            }
            var success = Students.TryRemove(request.Id, out _);
            if (!success)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Student not found"));
            }
            return Task.FromResult(new DeleteStudentResponse { Success = success });
        }
    }
}
