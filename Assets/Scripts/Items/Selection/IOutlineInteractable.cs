using UnityEngine;

public interface IOutlineInteractable
{
    /// <summary>
    /// Возвращает данные для отображения в overlay при наведении
    /// </summary>
    /// <param name="overlayInfo">Менеджер overlay информации</param>
    void ShowOverlayInfo(OverlayInfoManager overlayInfo);

    /// <summary>
    /// Обрабатывает клик по объекту
    /// </summary>
    /// <returns>True если клик был обработан, иначе false</returns>
    bool OnClick();
}