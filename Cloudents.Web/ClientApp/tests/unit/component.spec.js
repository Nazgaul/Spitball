import { mutations } from '../../store/Toaster'
const { setComponent } = mutations
console.log(mutations);

describe('set component mutations', () => {
    it('Should set component name', () => {
        const state = { component: ''}
        setComponent(state, 'component')

        expect(state.component).not.toBe('')
    })
})