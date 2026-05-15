/*
 * MIT License
 *Copyright (c) 2021 易开元(Easy-Open-Meta)

 *Permission is hereby granted, free of charge, to any person obtaining a copy
 *of this software and associated documentation files (the "Software"), to deal
 *in the Software without restriction, including without limitation the rights
 *to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *copies of the Software, and to permit persons to whom the Software is
 *furnished to do so, subject to the following conditions:

 *The above copyright notice and this permission notice shall be included in all
 *copies or substantial portions of the Software.

 *THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *SOFTWARE.
 *
 */
using EOM.TSHotelManagement.Contract;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 酒店宣传联动内容接口
    /// </summary>
    public interface IPromotionContentService
    {
        /// <summary>
        /// 查询所有宣传联动内容
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadPromotionContentOutputDto> SelectPromotionContentAll(ReadPromotionContentInputDto readPromotionContentInputDto);

        /// <summary>
        /// 查询所有宣传联动内容(跑马灯)
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadPromotionContentOutputDto> SelectPromotionContents();

        /// <summary>
        /// 添加宣传联动内容
        /// </summary>
        /// <param name="createPromotionContentInputDto"></param>
        /// <returns></returns>
        BaseResponse AddPromotionContent(CreatePromotionContentInputDto createPromotionContentInputDto);

        /// <summary>
        /// 删除宣传联动内容
        /// </summary>
        /// <param name="deletePromotionContentInputDto"></param>
        /// <returns></returns>
        BaseResponse DeletePromotionContent(DeletePromotionContentInputDto deletePromotionContentInputDto);

        /// <summary>
        /// 更新宣传联动内容
        /// </summary>
        /// <param name="updatePromotionContentInputDto"></param>
        /// <returns></returns>
        BaseResponse UpdatePromotionContent(UpdatePromotionContentInputDto updatePromotionContentInputDto);
    }
}