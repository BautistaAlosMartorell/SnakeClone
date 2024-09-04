using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.right;
    private Vector2 _queuedDirection = Vector2.right;
    private List<Transform> _segments = new List<Transform>();
    public Transform segmentPrefab;
    public int initialSize = 4;
    public int score;
    public Text scoreText;
    public Text scoreTextGO;
    public GameObject gameOverScreen;
    public Text highScoreTextGO;
    [SerializeField]
    public Food food;
    public GameObject particlesPrefab;

    private void Start()
    {
        ResetState();
        gameOverScreen.SetActive(false);
       

    }
    public void AddScore()
    {
        score = score + 1;
        scoreText.text=score.ToString();

        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _queuedDirection = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _queuedDirection = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            _queuedDirection = Vector2.right;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _queuedDirection = Vector2.left;
        }
    }

    private void FixedUpdate()
    {
        // Actualizar la dirección solo cuando la cabeza se mueve
        if (_queuedDirection != -_direction)
        {
            _direction = _queuedDirection;
        }

        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
          
        }

        this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x) + _direction.x,
            Mathf.Round(this.transform.position.y) + _direction.y,
            0.0f);
    }

    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;
        segment.GetComponent<SpriteRenderer>().color = GetRandomGreenColor();
        _segments.Add(segment);

    }
    private Color GetRandomGreenColor()
    {
        float r = Random.Range(0.0f, 0.2f); // Controla el rango del rojo
        float g = Random.Range(0.5f, 1.0f); // Controla el rango del verde
        float b = Random.Range(0.0f, 0.2f); // Controla el rango del azul
        return new Color(r, g, b);
    }

    private void ResetState()
    {
        for (int i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }
        _segments.Clear();
        _segments.Add(this.transform);
        for (int i = 1; i < this.initialSize; i++)
        {
            _segments.Add(Instantiate(this.segmentPrefab));
            
        }
        this.transform.position = Vector3.zero;
        _direction = Vector2.right;
        _queuedDirection = Vector2.right;
        food.RandomizePosition();
        Time.timeScale= 1.0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            Grow();
            AddScore();
            GameObject particles = Instantiate(particlesPrefab, transform.position, Quaternion.identity);
            particles.GetComponent<ParticleSystem>().Play();

        }
        else if (other.tag == "Obstacle")
        {
            ResetState();
            gameOver();
        }
    }
    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
    public void gameOver()
    {
        gameOverScreen.SetActive(true);
        scoreTextGO.text = score.ToString(); // Muestra el puntaje final
        highScoreTextGO.text = PlayerPrefs.GetInt("HighScore", 0).ToString(); // Muestra el mejor puntaje
        Time.timeScale = 0.0f;

    }
   
}
