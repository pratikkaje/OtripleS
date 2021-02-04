﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using OtripleS.Web.Api.Models.CourseAttachments;
using OtripleS.Web.Api.Models.CourseAttachments.Exceptions;
using Xunit;

namespace OtripleS.Web.Api.Tests.Unit.Services.CourseAttachments
{
    public partial class CourseAttachmentServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidatonExceptionOnRemoveWhenCourseIdIsInvalidAndLogItAsync()
        {
            // given
            Guid randomAttachmentId = Guid.NewGuid();
            Guid randomCourseId = default;
            Guid inputAttachmentId = randomAttachmentId;
            Guid inputCourseId = randomCourseId;

            var invalidCourseAttachmentInputException = new InvalidCourseAttachmentException(
                parameterName: nameof(CourseAttachment.CourseId),
                parameterValue: inputCourseId);

            var expectedCourseAttachmentValidationException =
                new CourseAttachmentValidationException(invalidCourseAttachmentInputException);

            // when
            ValueTask<CourseAttachment> removeCourseAttachmentTask =
                this.courseAttachmentService.RemoveCourseAttachmentByIdAsync(inputCourseId, inputAttachmentId);

            // then
            await Assert.ThrowsAsync<CourseAttachmentValidationException>(() =>
                removeCourseAttachmentTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedCourseAttachmentValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCourseAttachmentByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCourseAttachmentAsync(It.IsAny<CourseAttachment>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
