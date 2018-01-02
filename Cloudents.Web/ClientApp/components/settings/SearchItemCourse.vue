<template>
    <div class="course pa-2" :key="item.id" @click="$_updateMyCourses(item)">
        <input type="checkbox" :checked="isChecked"  :id="`course-${item.id}`" @change="$_updateMyCourses(item)" class="hidden-md-and-up"/>
        <label class="name" :title="item.name" :for="`course-${item.id}`">
            {{item.name | ellipsis(38)}}
        </label>
    </div>
    </template>
<script>
        import { mapGetters, mapMutations } from 'vuex'
        export default {
            model: {
                prop: 'value',
                event: 'selected'
            },
            props: { item: { type: Object, required: true }, value: {},isChecked:{type:Boolean,default:false} },

            computed: {
                ...mapGetters(['myCoursesId','myCourses']),
                checked: function () { return this.myCoursesId.includes(this.item.id)}
            },

            methods: {
                ...mapMutations({updateUser:'UPDATE_USER'}),
                $_updateMyCourses(val) {
                    let courses = this.myCourses
                    this.checked ? courses=courses.filter(i => i.id !== val.id) : courses.push(val);
                    this.updateUser({ myCourses: courses });
                }
            }
    }
    </script>