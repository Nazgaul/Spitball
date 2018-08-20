import { LanguageService } from './languageService'
/*
Hook functions (all optional):

bind: 
called only once, when the directive is first bound to the element. This is where you can do one-time setup work.

inserted: 
called when the bound element has been inserted into its parent node (this only guarantees parent node presence, not necessarily in-document).

update: 
called after the containing component’s VNode has updated, but possibly before its children have updated. The directive’s value may or may not have changed, but you can skip unnecessary updates by comparing the binding’s current and old values (see below on hook arguments).

componentUpdated: 
called after the containing component’s VNode and the VNodes of its children have updated.

unbind: 
called only once, when the directive is unbound from the element.

*/

// <span shor="a" title="s" v-language:inner,title,shor>lalala</span>
export const Language = {
    bind: function(el, binding){
        const INNER_HTML = "inner";

        //split arguments with delimeter ','
        let arrArgs = binding.arg.split(",");

        //iterate threw all args
        arrArgs.forEach(rawAttr => {
            let attr = rawAttr.trim();
            let keyValue = null;
            let key = null;
            //inner arg will replace the inner HTML
            if(attr === INNER_HTML){
                key = LanguageService.getValueByKey(el.innerHTML);
                el.innerHTML = key
            }else{
                //other will set the 
                keyValue = el.getAttribute(attr);
                key = LanguageService.getValueByKey(keyValue);
                el.setAttribute(attr, key);
            }
        });
    }
}