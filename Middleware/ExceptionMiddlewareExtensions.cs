using Microsoft.AspNetCore.Diagnostics;
using SchoolERP.Exceptions;

namespace SchoolERP.Middleware
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                    int statusCode = exception switch
                    {
                        UserNotFoundException                           => 404,
                        AdminNotFoundException                          => 404,
                        TeacherNotFoundException                        => 404,
                        StudentNotFoundException                        => 404,
                        InvalidCredentialsException                     => 401,
                        UserInactiveException                           => 403,
                        AdminAlreadyExistsException                     => 409,
                        SamePasswordException                           => 400,
                        IncorrectPasswordException                      => 400,
                        StudentClassNotFoundException                   => 404,
                        StudentClassAlreadyExistsException              => 409,
                        TeacherAlreadyAssignedAsClassTeacherException   => 409,
                        AttendanceAlreadyMarkedException                => 409,
                        InvalidAttendanceStatusException                => 400,
                        SubjectAlreadyExistsException                   => 409,
                        SubjectNotFoundException                        => 404,
                        HomeworkAlreadyExistsException                  => 409,
                        UnauthorizedSubjectAccessException              => 403,
                        FeeNotFoundException                            => 404,
                        FeeAlreadyPaidException                         => 409,
                        InvalidDueDateException                         => 400,
                        NoStudentsInClassException                      => 404,
                        FeeNotBelongToStudentException                  => 403,
                        NotAClassTeacherException                       => 403,
                        AttendanceStrengthMismatchException             => 400,
                        InvalidAttendanceValueException                 => 400,
                        RollNumberNotAssignedException                  => 400,
                        MarkListStrengthMismatchException               => 400,
                        MarksOutOfRangeException                        => 400,
                        MarksAlreadyEnteredForExamException             => 409,
                        AdminAttendanceAlreadyMarkedException           => 409,
                        MarkRecordNotFoundException                     => 404,
                        ClassTimetableAlreadyExistsException            => 409,
                        TeacherTimetableAlreadyExistsException          => 409,
                        SameTimeTableException                          => 409,
                        _                                               => 500
                    };

                    context.Response.StatusCode = statusCode;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        statusCode,
                        error = exception?.Message,
                        innerError = exception?.InnerException?.Message,
                        innerInnerError = exception?.InnerException?.InnerException?.Message
                    });
                });
            });
        }
    }
}