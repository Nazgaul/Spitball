<template>
    <div>Hi,{{id}}, {{item?item.name:'yifat'}}</div>
</template>
<script>
    import { mapActions } from 'vuex'
    export default {
        data() {
            return {
                item:null
            }
        },
        methods: {
            ...mapActions(['getPreview'])
        }, 

        created() {
            this.getPreview({ type: 'item', id: this.id }).then(res => {
                console.log(res)
                this.item =res
            })
        },
        props: { id: {type:String}}
    }
</script>