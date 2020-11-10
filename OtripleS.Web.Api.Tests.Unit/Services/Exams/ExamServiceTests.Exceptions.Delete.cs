﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE AS LONG AS SOFTWARE FUNDS ARE DONATED TO THE POOR
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Moq;
using OtripleS.Web.Api.Models.Exams;
using OtripleS.Web.Api.Models.Exams.Exceptions;
using Xunit;

namespace OtripleS.Web.Api.Tests.Unit.Services.Exams
{
	public partial class ExamServiceTests
	{
		[Fact]
		public async Task ShouldThrowDependencyExceptionOnDeleteWhenSqlExceptionOccursAndLogItAsync()
		{
			// given
			Guid randomExamId = Guid.NewGuid();
			Guid inputExamId = randomExamId;
			SqlException sqlException = GetSqlException();

			var expectedExamDependencyException =
				new ExamDependencyException(sqlException);

			this.storageBrokerMock.Setup(broker =>
				broker.SelectExamByIdAsync(inputExamId))
					.ThrowsAsync(sqlException);

			// when
			ValueTask<Exam> deleteExamTask =
				this.examService.DeleteExamByIdAsync(inputExamId);

			// then
			await Assert.ThrowsAsync<ExamDependencyException>(() =>
				deleteExamTask.AsTask());

			this.loggingBrokerMock.Verify(broker =>
				broker.LogCritical(It.Is(SameExceptionAs(expectedExamDependencyException))),
					Times.Once);

			this.storageBrokerMock.Verify(broker =>
				broker.SelectExamByIdAsync(inputExamId),
					Times.Once);

			this.dateTimeBrokerMock.VerifyNoOtherCalls();
			this.loggingBrokerMock.VerifyNoOtherCalls();
			this.storageBrokerMock.VerifyNoOtherCalls();
		}
	}
}
