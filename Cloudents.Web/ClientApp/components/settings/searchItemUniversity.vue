<template>
    <div class="uni ma-3" :key="item.id" @click="$_universitySelected(item)">
        <div class="image" :style="`background-image: url('${item.image}')`"></div>
        <div>{{item.name}}</div>
    </div>
</template>
<script>
    import { mapActions } from 'vuex'
    export default {
        props: { item: { type: Object, required: true } },
        methods: {
            ...mapActions(['updateUniversity']),
            $_universitySelected(val) {
                this.updateUniversity(val);
            }
        }
    }
</script>