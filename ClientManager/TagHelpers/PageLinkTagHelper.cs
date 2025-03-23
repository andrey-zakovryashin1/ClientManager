using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ClientManager.Models;

namespace ClientManager.TagHelpers
{
    /// <summary>
    /// A tag helper for generating pagination links.
    /// </summary>
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        private readonly ILogger<PageLinkTagHelper> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageLinkTagHelper"/> class.
        /// </summary>
        /// <param name="helperFactory">The URL helper factory for generating URLs.</param>
        /// <param name="logger">The logger for recording errors and information.</param>
        public PageLinkTagHelper(IUrlHelperFactory helperFactory, ILogger<PageLinkTagHelper> logger)
        {
            urlHelperFactory = helperFactory;
            this.logger = logger;
        }

        /// <summary>
        /// Gets or sets the view context.
        /// </summary>
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        /// <summary>
        /// Gets or sets the pagination model.
        /// </summary>
        public PageViewModel? PageModel { get; set; }

        /// <summary>
        /// Gets or sets the action name for generating pagination links.
        /// </summary>
        public string PageAction { get; set; } = "";

        /// <summary>
        /// Gets or sets the dictionary of URL values for generating pagination links.
        /// </summary>
        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; } = new();

        /// <summary>
        /// Processes the tag helper to generate pagination links.
        /// </summary>
        /// <param name="context">The context for the tag helper.</param>
        /// <param name="output">The output for the tag helper.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <see cref="PageModel"/> is not set.
        /// </exception>
        /// <exception cref="ApplicationException">
        /// Thrown when an unexpected error occurs during tag helper processing.
        /// </exception>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            try
            {
                if (PageModel == null)
                {
                    throw new ArgumentNullException(nameof(PageModel), "PageModel is not set.");
                }

                IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
                output.TagName = "div";

                TagBuilder tag = new TagBuilder("ul");
                tag.AddCssClass("pagination");

                TagBuilder currentItem = CreateTag(PageModel.PageNumber, urlHelper);

                if (PageModel.HasPreviousPage)
                {
                    TagBuilder prevItem = CreateTag(PageModel.PageNumber - 1, urlHelper);
                    tag.InnerHtml.AppendHtml(prevItem);
                }
                tag.InnerHtml.AppendHtml(currentItem);
                if (PageModel.HasNextPage)
                {
                    TagBuilder nextItem = CreateTag(PageModel.PageNumber + 1, urlHelper);
                    tag.InnerHtml.AppendHtml(nextItem);
                }
                output.Content.AppendHtml(tag);
            }
            catch (ArgumentNullException ex)
            {
                logger.LogError(ex, "PageModel is not set.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while processing the PageLinkTagHelper.");
                throw new ApplicationException("An unexpected error occurred while processing the PageLinkTagHelper.", ex);
            }
        }

        /// <summary>
        /// Creates a pagination tag for the specified page number.
        /// </summary>
        /// <param name="pageNumber">The page number for the tag.</param>
        /// <param name="urlHelper">The URL helper for generating the link.</param>
        /// <returns>The created tag.</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when an error occurs while creating a pagination tag.
        /// </exception>
        TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper)
        {
            try
            {
                TagBuilder item = new TagBuilder("li");
                TagBuilder link = new TagBuilder("a");
                if (pageNumber == PageModel?.PageNumber)
                {
                    item.AddCssClass("active");
                }
                else
                {
                    PageUrlValues["page"] = pageNumber;
                    link.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                }
                item.AddCssClass("page-item");
                link.AddCssClass("page-link");
                link.InnerHtml.Append(pageNumber.ToString());
                item.InnerHtml.AppendHtml(link);
                return item;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating a pagination tag.");
                throw new ApplicationException("An error occurred while creating a pagination tag.", ex);
            }
        }
    }
}
