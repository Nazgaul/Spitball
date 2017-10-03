<template> 
        <item-result :items="items" ></item-result>
</template>
<script>
    import itemResult from './ResultItem.vue'
    import { itemsList } from './mixins'

    export default {
        mixins: [itemsList],
        components: { itemResult }
    }
</script>