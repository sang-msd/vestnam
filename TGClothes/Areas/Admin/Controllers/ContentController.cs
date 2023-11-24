using Data.EF;
using Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TGClothes.Areas.Admin.Controllers
{
    public class ContentController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly IContentService _contentService;

        public ContentController(ICategoryService categoryService, IContentService contentService)
        {
            _categoryService = categoryService;
            _contentService = contentService;
        }

        // GET: Admin/Content
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var model = _contentService.GetAllPaging(searchString, page, pageSize);
            ViewBag.SearchString = searchString;
            return View(model);
        }

        public ActionResult Create()
        {
            SetViewbag();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Content content)
        {
            if (ModelState.IsValid)
            {
                _contentService.Create(content);
                return RedirectToAction("Index");
            }
            SetViewbag();
            return View();
        }

        public ActionResult Edit(long id)
        {
            var content = _contentService.GetById(id);
            SetViewbag(content.CategoryId);
            return View(content);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Content content)
        {
            if (ModelState.IsValid)
            {
                var result = _contentService.Edit(content);
                if (result > 0)
                {
                    SetAlert("Cập nhật tin tức thành công", "success");
                    return RedirectToAction("Index", "Content");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật tin tức không thành công");
                }
            }
            SetViewbag(content.CategoryId);
            return View("Index");
        }

        public void SetViewbag(long? selectedId = null)
        {
            ViewBag.CategoryId = new SelectList(_categoryService.GetAll(), "Id", "Name", selectedId);
        }
    }
}