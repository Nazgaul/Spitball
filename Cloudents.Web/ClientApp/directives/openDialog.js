

function _openDialog(el){   
   let target = el.target;
   let options;
   // eslint-disable-next-line no-constant-condition
   while (true) {
      if (!target) {
         break;
      }
      options = target.options;
      if (options) {
         break;
      }
      target = target.parentElement;
   }
   if (options) {
      target.openDialog(options)
   }
}

export const openDialog = {
   bind: function(el, binding,vnode) {
      el.options = binding.value;
      el.openDialog = vnode.componentInstance.$openDialog
      el.addEventListener('click', _openDialog );
   },
   unbind: function(el) {
       el.removeEventListener('click',_openDialog);
   },
}