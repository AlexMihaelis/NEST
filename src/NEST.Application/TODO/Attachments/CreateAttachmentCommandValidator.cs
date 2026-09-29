using FluentValidation;

namespace NEST.Application.TODO.Attachments;

// Проверяем основные данные файла перед его загрузкой в хранилище
public class CreateAttachmentCommandValidator : AbstractValidator<CreateAttachmentCommand>
{
    // Максимальный размер одного файла - 10 МБ
    private const long MaxFileSize = 10 * 1024 * 1024;

    public CreateAttachmentCommandValidator()
    {
        // Пользователь, загрузивший файл, должен быть указан
        RuleFor(a => a.UploadedByUserId)
            .NotEmpty();

        // Файл должен содержать данные и не превышать максимальный размер
        RuleFor(a => a.FileStream)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(stream => stream.Length > 0)
            .WithMessage("The file stream is empty.")
            .Must(stream => stream.Length <= MaxFileSize)
            .WithMessage("File size must not exceed 10 MB.");

        // Имя файла обязательно и не превышает ограничение Attachment - 255 символов
        RuleFor(a => a.FileName)
            .NotEmpty()
            .MaximumLength(255);

        // MIME-тип обязателен и не превышает 100 символов
        RuleFor(a => a.ContentType)
            .NotEmpty()
            .MaximumLength(100);
    }
}