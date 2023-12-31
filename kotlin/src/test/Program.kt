import org.assertj.core.api.Assertions.assertThat
import org.junit.jupiter.api.BeforeEach
import org.junit.jupiter.api.DisplayName
import org.junit.jupiter.api.Test
 
class ProgramTest {
 
    private var expected: Int = 0
 
    @BeforeEach
    fun configureSystemUnderTest() {
        expected = 40 + 2
    }
 
    @Test
    @DisplayName("Should return the correct message")
    fun shouldReturnCorrectMessage() {
        assertThat(expected).isEqualTo(42)
    }
}