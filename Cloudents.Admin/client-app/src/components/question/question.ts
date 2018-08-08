import Vue from 'vue'
import { Component } from 'vue-property-decorator'

@Component({})
export default class Question extends Vue{
    msg: string = "Hello From question"
}