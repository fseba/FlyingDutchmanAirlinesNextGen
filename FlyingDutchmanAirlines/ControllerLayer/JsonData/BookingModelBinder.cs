using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FlyingDutchmanAirlines.ControllerLayer.JsonData;

public class BookingModelBinder : IModelBinder
{
  public BookingModelBinder()
  {
  }

  public async Task BindModelAsync(ModelBindingContext bindingContext)
  {
    if (bindingContext == null)
    {
      throw new ArgumentException();
    }

    ReadResult result = await bindingContext.HttpContext.Request.BodyReader.ReadAsync();

    ReadOnlySequence<byte> buffer = result.Buffer;
    string body = Encoding.UTF8.GetString(buffer.FirstSpan);

    BookingData? data = JsonSerializer.Deserialize<BookingData>(body);

    bindingContext.Result = ModelBindingResult.Success(data);
  }
}

